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
    if (this.stateService.devices.size > 0) return;
    const call = this.http.get<Device[]>(environment.restBaseUrl + "/device")
    const response = await firstValueFrom<Device[]>(call);
    response.forEach(device => {
      this.stateService.devices.set(device.mac, device);
    })
    return response;
  }

  async getAllBmeData() {
    if (this.stateService.devices.size === 0) {
      await this.getDevices();
    }
    this.stateService.devices.forEach((_, mac) => this.getBmeData(mac));
  }

  async getBmeData(mac: string) {
    this.ws.sendJson(new ClientStartsListeningToDevice({mac: mac}));
  }
}
