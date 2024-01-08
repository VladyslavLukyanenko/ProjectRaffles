const relay = window.__PROJECTRAFFLES_CEF_RELAY || ({
  notify: (args) => {
    console.log(`Notified with args`, args)
    return Promise.resolve(null)
  }
});

let mapProvider;
const cefIntegration = map => {
  if (window.map === map) {
    return;
  }

  window.map = map;
  mapProvider = () => map;
  console.log("Settings up cef integration");
  map.on('load', function () {
    ping({
      "type": "load",
    });
  });


  //
  // var currentCenter = null;
  // var currentZoom = null;
  // var currentPitch = null;
  // var currentBearing = null;
  //
  // map.on("render", function () {
  //   var newCenter = map.getCenter();
  //
  //   if (currentCenter === null || currentCenter.lat != newCenter.lat || currentCenter.lng != newCenter.lng) {
  //     currentCenter = newCenter;
  //     ping({
  //       "type": "move",
  //       "center": newCenter,
  //     });
  //   }
  //
  //   var newZoom = map.getZoom();
  //
  //   if (currentZoom === null || currentZoom != newZoom) {
  //     currentZoom = newZoom;
  //     ping({
  //       "type": "zoom",
  //       "zoom": newZoom,
  //     });
  //   }
  //
  //   var newPitch = map.getPitch();
  //
  //   if (currentPitch === null || currentPitch != newPitch) {
  //     currentPitch = newPitch;
  //     ping({
  //       "type": "pitch",
  //       "pitch": newPitch,
  //     });
  //   }
  //
  //   var newBearing = map.getBearing();
  //   if (currentBearing === null || currentBearing != newBearing) {
  //     currentBearing = newBearing;
  //     ping({
  //       "type": "bearing",
  //       "bearing": newBearing,
  //     });
  //   }
  // });
  //
  // map.on("mousedown", function () {
  //   ping({
  //     "type": "mouseDown",
  //   });
  // });
  //
  // map.on("mousemove", function () {
  //   ping({
  //     "type": "mouseMove",
  //   });
  // });
  //
  // map.on("mouseup", function () {
  //   ping({
  //     "type": "mouseUp",
  //   });
  // });
  //
  // map.on("mouseenter", "", function () {
  //   ping({
  //     "type": "mouseEnter",
  //   });
  // });
  //
  // map.on("mouseleave", function () {
  //   ping({
  //     "type": "mouseLeave",
  //   });
  // });
  //
  // map.on("click", function () {
  //   ping({
  //     "type": "click",
  //   });
  // });
  //
  // map.on("dblclick", function () {
  //   ping({
  //     "type": "doubleClick",
  //   });
  // });
}

export const integrations = {
  dispatchMapReady: map => {
    window.map = map;
    ping({
      "type": "ready",
      "path": window.location.href,
    });
  },

  dispatchZoomChanged: newZoom => ping({
    "type": "zoom",
    "zoom": newZoom,
  }),

  dispatchBearingChanged: value => ping({
    "type": "bearing",
    "value": value,
  }),

  dispatchPitchChanged: value => ping({
    "type": "pitch",
    "value": value,
  }),

  dispatchSelectionChanged: (center, radiusKm) => ping({
    "type": "selection",
    "value": {center, radiusKm},
  }),
}

function ping(data) {

  //parentMap.notify(JSON.stringify(data));

  relay.notify(JSON.stringify(data)).then(function (res) {

  });
}


function exec(expression) {
  var result = eval(expression);

  try {
    return JSON.stringify(result);
  } catch (e) {
    return "null";
  }
}

function run(expression) {
  var f = new Function("(function() { " + expression + " })()");
  f();
}

function addImage(id, base64) {
  var img = new Image();
  img.onload = function () {
    mapProvider().addImage(id, img);
  }
  img.onerror = function (errorMsg, url, lineNumber, column, errorObj) {
    ping({
      "type": "error",
      "info": errorMsg,
    });
  }

  img.src = "data:image/png;base64," + base64;
}

window.exec = exec;
window.run = run;
window.addImage = addImage;

export default cefIntegration;