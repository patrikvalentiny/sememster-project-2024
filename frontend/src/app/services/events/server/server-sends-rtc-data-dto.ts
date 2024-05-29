import {BaseDto} from "../base-dto";
import {BmeData} from "../../../models/bme-data";

export class ServerSendsRtcDataDto extends BaseDto<ServerSendsRtcDataDto> {
    mac?: string;
    data?: BmeData;
}
