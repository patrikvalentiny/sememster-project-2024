import {inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Device} from "../models/device";
import {environment} from "../../environments/environment";
import {firstValueFrom} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  devices:Map<string, Device> = new Map<string, Device>();
  private readonly http: HttpClient = inject(HttpClient);

  constructor() {
    this.getDevices().then(() => {
    })
  }


  async getDevices() {
    const call = this.http.get<Device[]>(environment.restBaseUrl + "/device")
    const response = await firstValueFrom<Device[]>(call);
    response.forEach(device => {
      this.devices.set(device.mac, device);
    })
    return this.devices;
  }
}
