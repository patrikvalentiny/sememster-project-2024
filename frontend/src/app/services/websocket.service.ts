import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import ReconnectingWebSocket from "reconnecting-websocket";

@Injectable({
  providedIn: 'root'
})
export class WebsocketService {
  private readonly rws : ReconnectingWebSocket = new ReconnectingWebSocket(environment.wsBaseUrl);
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
}
