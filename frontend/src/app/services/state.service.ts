import {Injectable} from '@angular/core';
import {Device} from "../models/device";
import {BmeData} from "../models/bme-data";

@Injectable({
  providedIn: 'root'
})
export class StateService {
  devices:Map<string, Device> = new Map<string, Device>();
  bmeData:Map<string, BmeData[]> = new Map<string, BmeData[]>();
  motorPosition:Map<string, number> = new Map<string, number>();
  constructor() {

  }
}
