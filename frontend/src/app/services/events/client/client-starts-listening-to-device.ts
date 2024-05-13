import {BaseDto} from "../base-dto";

export class ClientStartsListeningToDevice extends BaseDto<ClientStartsListeningToDevice> {
  mac?:string;
}
