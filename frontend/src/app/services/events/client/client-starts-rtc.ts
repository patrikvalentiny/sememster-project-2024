import {BaseDto} from "../base-dto";

export class ClientStartsRtc extends BaseDto<ClientStartsRtc>{
  mac?: string;
}
export class ClientEndsRtc extends BaseDto<ClientEndsRtc>{
  mac?: string;
}
