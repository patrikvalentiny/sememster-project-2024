import {Device} from "../../../models/device";
import {BaseDto} from "../base-dto";

export class ServerDeviceOnline extends BaseDto<ServerDeviceOnline> {
  device?: Device;
}
