import React from "react";
import ReactMapboxGl, {MapContext} from "react-mapbox-gl";
import MapboxDraw from "@mapbox/mapbox-gl-draw";
import "@mapbox/mapbox-gl-draw/dist/mapbox-gl-draw.css";
import {CircleMode, DirectMode, DragCircleMode, SimpleSelectMode,} from "mapbox-gl-draw-circle";
import cefIntegration, {integrations} from "./cef-integration";

const ReactMap = ReactMapboxGl({
  accessToken: window.__PROJECTRAFFLES_ACCESSTOKEN || "pk.eyJ1IjoiYWxhbnRvbyIsImEiOiJja2g1M3Z3d3IwdXJsMnRsczB4ZDBzMHRiIn0.tP4cOCUfBr5RKnUMIyXnag"
});

const mapboxProps = {
  style: "mapbox://styles/mapbox/streets-v11",
  zoom: [13],
  containerStyle: {
    height: "100vh",
    width: "100%",
  }
};


class Map extends React.Component {
  constructor (props) {
    super(props);
    this.draw = new MapboxDraw({
      displayControlsDefault: false,
      userProperties: true,
      defaultMode: "draw_circle",
      clickBuffer: 10,
      touchBuffer: 10,
      modes: {
        ...MapboxDraw.modes,
        draw_circle: CircleMode,
        // direct_select: DirectMode,
        // simple_select: SimpleSelectMode,
        // drag_circle: DragCircleMode
      }
    });

    // this.what = function() {
    //   console.log(this.draw.getAll().features);
    //   setTimeout(this.what, 2000);
    // }.bind(this);
  }

  componentDidUpdate(prevProps, prevState, snapshot) {
    if (prevProps.mode !== this.props.mode ||
        this.props.features.length !== this.draw.getAll().features.length) {
      // console.log(this.props.mode);
      this.draw.deleteAll();
      this.draw.changeMode(this.props.mode);
    }
  }

  onMapLoaded = map => {
    map.addControl(this.draw);
    integrations.dispatchMapReady(map);
    // this.what();

    const notifyParent = (features) => this.props.onFeaturesUpdated(features);
    map.on("draw.create", e => notifyParent(e.features));
    map.on("draw.update", e => notifyParent(e.features));
    map.on("draw.delete", e => notifyParent(e.features));

  };

  render() {
    return (
      <div>
        <ReactMap
          {...mapboxProps}
          onStyleLoad={map => this.onMapLoaded(map)}
          onPitch={v => integrations.dispatchPitchChanged(v.getPitch())}
          onRotate={v => integrations.dispatchBearingChanged(v.getBearing())}
          onZoom={v => integrations.dispatchZoomChanged(v.getZoom())}
        >
          <MapContext.Consumer>
            {
              (map) => cefIntegration(map)
            }
          </MapContext.Consumer>
        </ReactMap>
      </div>
    );
  }
}

export default Map;
