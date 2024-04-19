import {inject, Injectable} from '@angular/core';
import {environment} from "../../environments/environment";
import ReconnectingWebSocket from "reconnecting-websocket";
import { ServerSendsNotification } from './events/server-sends-notification';
import {HotToastService} from "@ngxpert/hot-toast";

@Injectable({
  providedIn: 'root'
})
export class WebsocketService {
  private readonly rws : ReconnectingWebSocket = new ReconnectingWebSocket(environment.wsBaseUrl);
  private readonly toast = inject(HotToastService);

  online: boolean = false;
  constructor() {
    this.rws.addEventListener("open", () => {
      this.online = true;
    });
    this.rws.addEventListener("close", () => {
      this.online = false;
    });
    this.rws.addEventListener("message", message => {
      console.log("Received message: ", message.data);
    });
    }

    send(message: string){
      this.rws.send(message);
    }

  private handleEvent(event: MessageEvent) {
    console.log("Received: " + event.data);
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
}

export class BaseDto<T> {
  eventType: string;

  constructor(init?: Partial<T>) {
    this.eventType = this.constructor.name;
    Object.assign(this, init);
  }
}
