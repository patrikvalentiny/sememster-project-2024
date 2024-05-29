import {BaseDto} from "../base-dto";

export class ClientStartsRtc extends BaseDto<ClientStartsRtc>{
  mac?: string;
}
export class ClientStopsRtc extends BaseDto<ClientStopsRtc>{
  mac?: string;
}
