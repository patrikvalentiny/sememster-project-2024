import {inject, Injectable, signal, WritableSignal} from '@angular/core';
import {environment} from "../../environments/environment";
import ReconnectingWebSocket from "reconnecting-websocket";
import {ServerSendsNotification} from './events/server-sends-notification';
import {HotToastService} from "@ngxpert/hot-toast";
import {ServerDeviceOnline} from "./events/server/server-device-online";
import {BaseDto} from "./events/base-dto";
import {ServerDeviceBmeData} from './events/server/server-device-bme-data';
import {BmeData} from "../models/bme-data";
import {ServerSendsDeviceBaseDataDto} from './events/server/server-sends-device-base-data-dto';
import {StateService} from "./state.service";
import {ServerSendsMotorDataDto} from "./events/server/server-sends-motor-data-dto";
import {ServerSendsRtcDataDto} from "./events/server/server-sends-rtc-data-dto";

@Injectable({
  providedIn: 'root'
})
export class WebsocketService {
  online: boolean = false;
  private readonly rws: ReconnectingWebSocket = new ReconnectingWebSocket(environment.wsBaseUrl);
  private readonly toast = inject(HotToastService);
  private readonly stateService = inject(StateService);

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
    if (!environment.production) {
      console.log("Sending: " + JSON.stringify(message));
    }
    this.rws.send(message);
  }

  sendJson(message: object) {
    this.send(JSON.stringify(message));
  }

  private handleEvent(event: MessageEvent) {
    if (!environment.production) {
      console.log("Received: " + event.data);
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
    this.stateService.devices.set(data.device!.mac, data.device!);
    this.toast.info(`Device ${data.device!.name} is online`);
  }

  private ServerDeviceBmeData(data: ServerDeviceBmeData) {
    const bmeData = data.data!;
    const bmeDataList = this.stateService.bmeData.get(bmeData.deviceMac!)!;
    bmeDataList.update(value => [bmeData as BmeData, ...value.filter(value => new Date(value.createdAt).getTime() > Date.now() - 24 * 60 * 60 * 1000)]);
  }

  private ServerSendsDeviceBaseData(data: ServerSendsDeviceBaseDataDto) {
    this.stateService.bmeData.set(data.mac!, signal(data.data!));
  }

  private ServerSendsMotorData(data: ServerSendsMotorDataDto) {
    this.stateService.motorPosition.set(data.mac!, data.position!);
    this.stateService.motorMoving.set(data.mac!, false);
  }

  private ServerSendsRtcData(data: ServerSendsRtcDataDto) {
    const signalList: WritableSignal<BmeData[]> | undefined = this.stateService.rtcData.get(data.mac!);
    if (!signalList) {
      this.stateService.rtcData.set(data.mac!, signal([data.data!]));
      return;
    }
    signalList.update(value => [data.data!, ...value]);
  }

}


