import {BaseDto} from "../base-dto";
import {BmeDataDto} from "./bme-data-dto";

export class ServerDeviceBmeData extends BaseDto<ServerDeviceBmeData> {
  data?: BmeDataDto;
}
