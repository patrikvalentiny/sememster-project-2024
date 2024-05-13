import {inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Device} from "../models/device";
import {environment} from "../../environments/environment";
import {firstValueFrom} from "rxjs";
import {WebsocketService} from "./websocket.service";
import {ClientStartsListeningToDevice} from "./events/client/client-starts-listening-to-device";
import {StateService} from "./state.service";

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly http: HttpClient = inject(HttpClient);
  private readonly ws = inject(WebsocketService);
  private readonly stateService = inject(StateService);

  constructor() {
    this.getDevices().then(() => {
    })
  }


  async getDevices() {
    const call = this.http.get<Device[]>(environment.restBaseUrl + "/device")
    const response = await firstValueFrom<Device[]>(call);
    response.forEach(device => {
      this.stateService.devices.set(device.mac, device);
      this.getBmeData(device.mac);
    })
    return response;
  }

  async getBmeData(mac: string) {
    this.ws.send(JSON.stringify(new ClientStartsListeningToDevice({mac: mac})));
  }
}
