# ClimateCtrl

Realtime data visualization of temperature and humidity data from a ESP32 in a dashboard.
### [StartWebSocketServer](api/StartupClass.cs)
- Registers Websocket handlers to:
    - OnOpen adds connection to stateful collection
    - OnClose removes connection from stateful collection and removes client any other places it is stored
    - OnMessage sends message to correct handler via MediatR
  
### [Frontend DashboardService](frontend/src/app/services/dashboard.service.ts)
- Fetches devices from the backend via REST API
- Fetches data from the backend via Websockets
  
### [InvokeBaseDtoHandler](api/Utils/WsHelper.cs)
- A helper class that invokes the correct handler for a given DTO via MediatR
- If the handler returns a response, it sends the response to the client

### [ClientStartsListeningToDevice](api/ClientEventHandlers/ClientStartsListeningToDevice.cs)
- An event handler that handles the event when a client starts listening to a device
- Sends the device data to the client limited to last 24 hours
- Registers the client to the device

### [Frontend ServerSendsDeviceBaseData](frontend/src/app/services/websocket.service.ts)
- A method that handles DTO that is sent from the server to the client when a device sends data
- Creates a new key (device mac) and value (signal(device data)) pair in the bmeData map

### [Frontend CardLineChartComponent](frontend/src/app/charts/card-line-chart/card-line-chart.component.ts)
- A component that displays a line chart
- Takes in a signal array and displays it in a chart
- Updates the chart when new data is received from the server
- Change detection is achieved by using signals

### [MqttDeviceDataClient](api/Mqtt/MqttDeviceDataClient.cs)
- A class that listens to MQTT messages from devices
- Parses the message and sends the data to the listening clients

### [Frontend ServerDeviceBmeData](frontend/src/app/services/websocket.service.ts)
- A method that handles DTO that is sent from the server to the client when a device sends data
- Updates the bmeData map with the new data




