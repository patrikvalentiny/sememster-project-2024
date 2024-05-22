import {BaseDto} from "../base-dto";
import {BmeData} from "../../../models/bme-data";

export class ServerSendsDeviceBaseDataDto extends BaseDto<ServerSendsDeviceBaseDataDto> {
  mac?: string;
  data?: BmeData[]
}
