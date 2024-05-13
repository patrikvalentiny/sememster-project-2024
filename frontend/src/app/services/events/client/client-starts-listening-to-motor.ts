import {ClientStartsListeningToDevice} from "./client-starts-listening-to-device";
import {BaseDto} from "../base-dto";

export class ClientStartsListeningToMotor extends BaseDto<ClientStartsListeningToDevice>{
  mac?: string;
}
