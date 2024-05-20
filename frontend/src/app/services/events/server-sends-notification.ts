import {BaseDto} from "./base-dto";


export class ServerSendsNotification extends BaseDto<ServerSendsNotification> {
  type?: string;
  message?: string;
}
