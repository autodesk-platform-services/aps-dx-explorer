import { initViewer, loadModel } from './viewer.js'

window.addEventListener("load", async () => {
  const login = document.getElementById('login');
  const urnInput = document.getElementById('modelurn');
  const viewerToggle = document.getElementById('toggleviewer');
  const viewerDiv = document.getElementById('viewer');
  const graphiqlDiv = document.getElementById('graphiql');
  new ResizeObserver(() => {
    viewerDiv.style.height = `calc( ${document.body.scrollHeight}px - (1em + ${graphiqlDiv.clientHeight}px))`;
    if (!!globalViewer)
      globalViewer.resize();
  }).observe(graphiqlDiv);
  initViewer(viewerDiv).then(viewer => {
    globalViewer = viewer;
  });
  viewerToggle.onclick = async (cb) => {
    try {
      if (cb.target.checked) {
        let itemId = urnInput.value;
        let exchangeInfo = await (await fetch(`/api/hubs/exchanges/${itemId}`)).json();
        let fileVersionUrn = '';
        let components = JSON.parse(exchangeInfo).results[0].components.data.insert;

        for (const key in components) {
          if (key.startsWith('autodesk.data:exchange.host')) {
            fileVersionUrn = findValue(components[key], "versionId");
          }
          if (key.startsWith('autodesk.fdx:host')) {
            fileVersionUrn = findValue(components[key], "versionUrn");
          }
        }

        await resizeGraphiql(graphiqlDiv, false);
        await loadNDisplayModel(graphiqlDiv, viewerDiv, globalViewer, fileVersionUrn);
      }
      else {
        hideModel(viewerDiv);
        await resizeGraphiql(graphiqlDiv, true);
      }
    } catch (error) {
      showToast(`Please, ensure you're logged in and using a valid item/version ID!`);
      showHelpers('iteminput');
      showHelpers('login');
      console.error(error);
      cb.target.checked = false;
    }
  }
  try {
    const resp = await fetch('/api/auth/profile');
    if (resp.ok) {
      const user = await resp.json();
      login.innerText = `Logout (${user.name})`;
      login.onclick = () => window.location.replace('/api/auth/logout');
    } else {
      login.innerText = 'Login';
      login.onclick = () => window.location.replace('/api/auth/login');
    }
    login.style.visibility = 'visible';
  } catch (err) {
    alert('Could not initialize the application. See console for more details.');
    console.error(err);
  }
})

function findValue(obj, targetKey) {
  for (let key in obj) {
    if (key === targetKey) {
      return obj[key];
    }

    if (typeof obj[key] === 'object') {
      const result = findValue(obj[key], targetKey);
      if (result !== undefined) {
        return result;
      }
    }
  }
  return undefined;
}

async function showToast(message) {
  Swal.fire({
    title: message,
    timer: 5000,
    toast: true,
    position: 'top',
    showConfirmButton: false
  })
}

async function resizeGraphiql(graphiqlDiv, increase) {
  if (increase) {
    graphiqlDiv.style.height = 'calc(100% - 3em)';
  }
  else {
    graphiqlDiv.style.height = 'calc(70%)';
  }
}

async function loadNDisplayModel(graphiqlDiv, viewerDiv, viewer, urn) {
  try {
    viewerDiv.style.visibility = 'visible';
    viewerDiv.style.height = `calc( ${document.body.scrollHeight}px - (1em + ${graphiqlDiv.clientHeight}px))`;
    viewer.resize();
    loadModel(viewer, btoa(urn)).then();
  }
  catch (err) {
    console.log(`Not able to load the model: ${err}`);
  }
}

async function hideModel(viewerDiv) {
  try {
    viewerDiv.style.visibility = 'hidden';
  }
  catch (err) {
    console.log(`Not able to load the model: ${err}`);
  }
}