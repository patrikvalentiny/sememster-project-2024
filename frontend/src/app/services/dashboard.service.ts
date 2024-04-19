import {inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Device} from "../models/device";
import {environment} from "../../environments/environment";
import {firstValueFrom} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class DashboardService{
  private readonly http: HttpClient = inject(HttpClient);
  devices:Device[] = [];
  constructor() {
    this.getDevices().then(() =>{})
  }


  async getDevices(){
    const call = this.http.get<Device[]>(environment.restBaseUrl + "/device")
    this.devices = await firstValueFrom<Device[]>(call);
    return this.devices;
  }
}
