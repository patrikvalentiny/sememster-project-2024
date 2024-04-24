import {BaseDto} from "../../websocket.service";
import {ServerSendsNotification} from "../server-sends-notification";
import {Device} from "../../../models/device";

export class ServerDeviceOnline extends BaseDto<ServerDeviceOnline> {
  device?: Device;
}
