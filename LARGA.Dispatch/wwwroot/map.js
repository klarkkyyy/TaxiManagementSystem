// Leaflet map helpers (OpenStreetMap tiles - free, no key).
window.larga = {
    maps: {},
    pins: {},
    line: null,

    // Teardrop pin drawn as inline SVG so it matches the app theme.
    pinIcon: function (color) {
        const svg = `<svg viewBox="0 0 32 42"><path d="M16 1C8 1 2 7 2 15c0 10 14 26 14 26s14-16 14-26C30 7 24 1 16 1z" fill="${color}" stroke="#fff" stroke-width="2"/><circle cx="16" cy="15" r="5" fill="#fff"/></svg>`;
        return L.divIcon({ html: svg, className: "pin", iconSize: [32, 42], iconAnchor: [16, 40], popupAnchor: [0, -36] });
    },

    createMap: function (elementId) {
        if (this.maps[elementId]) {
            this.maps[elementId].remove();
        }
        const map = L.map(elementId).setView([14.5995, 120.9842], 12); // Manila
        L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", {
            maxZoom: 19,
            attribution: "&copy; OpenStreetMap contributors"
        }).addTo(map);
        this.maps[elementId] = map;
        this.pins = {};
        this.line = null;
        return map;
    },

    // Pick locations: every click on the map is reported back to C# (OnMapClicked).
    initPicker: function (elementId, dotNetRef) {
        const map = this.createMap(elementId);
        map.on("click", e => dotNetRef.invokeMethodAsync("OnMapClicked", e.latlng.lat, e.latlng.lng));
    },

    // kind is "pickup" or "dropoff"; the line between the two pins is redrawn each time.
    setPin: function (elementId, kind, lat, lng) {
        const map = this.maps[elementId];
        if (!map) return;

        if (this.pins[kind]) {
            this.pins[kind].remove();
        }
        const color = kind === "pickup" ? "#019bcf" : "#1d1d1d";
        this.pins[kind] = L.marker([lat, lng], { icon: this.pinIcon(color) })
            .addTo(map)
            .bindPopup(kind === "pickup" ? "Pickup" : "Drop-off");

        if (this.line) {
            this.line.remove();
            this.line = null;
        }
        if (this.pins.pickup && this.pins.dropoff) {
            const a = this.pins.pickup.getLatLng();
            const b = this.pins.dropoff.getLatLng();
            this.line = L.polyline([a, b], { color: "#019bcf", weight: 4, dashArray: "8 8" }).addTo(map);
            map.fitBounds([a, b], { padding: [60, 60], maxZoom: 15 });
        }
    },

    flyTo: function (elementId, lat, lng) {
        const map = this.maps[elementId];
        if (map) map.flyTo([lat, lng], 16);
    },

    // Read-only map with one pin per point: [ { lat, lng, label } ]
    drawMap: function (elementId, points) {
        const map = this.createMap(elementId);
        const latLngs = points.map(p => [p.lat, p.lng]);
        points.forEach(p => L.marker([p.lat, p.lng], { icon: this.pinIcon("#019bcf") }).addTo(map).bindPopup(p.label));
        map.fitBounds(latLngs, { padding: [40, 40], maxZoom: 15 });
    }
};
