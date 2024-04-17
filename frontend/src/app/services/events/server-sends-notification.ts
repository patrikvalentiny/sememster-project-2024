import {BaseDto} from "../websocket.service";

export class ServerSendsNotification extends BaseDto<ServerSendsNotification> {
  type?: string;
  message?: string;
}
