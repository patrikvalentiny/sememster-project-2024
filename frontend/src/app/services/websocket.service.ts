import {inject, Injectable} from '@angular/core';
import {environment} from "../../environments/environment";
import ReconnectingWebSocket from "reconnecting-websocket";
import {ServerSendsNotification} from './events/server-sends-notification';
import {HotToastService} from "@ngxpert/hot-toast";
import {Device} from "../models/device";
import {ServerDeviceOnline} from "./events/server/server-device-online";
import {DashboardService} from "./dashboard.service";
import {BaseDto} from "./events/base-dto";
import { ServerDeviceBmeData } from './events/server/server-device-bme-data';
import {BmeData} from "../models/bme-data";
import {BmeDataDto} from "./events/server/bme-data-dto";

@Injectable({
  providedIn: 'root'
})
export class WebsocketService {
  online: boolean = false;
  private readonly rws: ReconnectingWebSocket = new ReconnectingWebSocket(environment.wsBaseUrl);
  private readonly toast = inject(HotToastService);
  private readonly dashboardService = inject(DashboardService);

  constructor() {
    this.rws.addEventListener("open", () => {
      this.online = true;
    });
    this.rws.addEventListener("close", () => {
      this.online = false;
    });
    this.rws.addEventListener("message", message => {
      this.handleEvent(message);
    });
  }

  send(message: string) {
    this.rws.send(message);
  }

  private handleEvent(event: MessageEvent) {
    if (!environment.production) {
      console.log("Message: " + event.data);
    }
    const data = JSON.parse(event.data) as BaseDto<any>;
    //@ts-ignore
    this[data.eventType].call(this, data);
  }

  private ServerSendsNotification(data: ServerSendsNotification) {
    switch (data.type) {
      case "error":
        this.toast.error(data.message);
        break;
      case "success":
        this.toast.success(data.message);
        break;
      case "info":
        this.toast.info(data.message);
        break;
      case "warning":
        this.toast.warning(data.message);
        break;
      default:
        this.toast.show(data.message)
    }
  }

  private ServerDeviceOnline(data: ServerDeviceOnline) {
    this.dashboardService.devices.set(data.device!.mac, data.device!);
    this.toast.info(`Device ${data.device!.name} is online`);
  }

  private ServerDeviceBmeData(data: ServerDeviceBmeData) {
    const bmeData = data.data!;
    bmeData.createdAt = new Date(Date.parse(bmeData!.createdAt!.toString() + "Z"))

    const bmeDataList = (this.dashboardService.bmeData.get(bmeData.deviceMac!) ?? []).slice(0, 12);
    bmeDataList.unshift(bmeData as BmeData);
    this.dashboardService.bmeData.set(bmeData.deviceMac!, bmeDataList);
  }

}


