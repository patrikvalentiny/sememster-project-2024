import {BaseDto} from "../base-dto";

export class ClientControlsMotor extends BaseDto<ClientControlsMotor>{
  mac?: string;
  steps?: number;
}
