import {Injectable, WritableSignal} from '@angular/core';
import {Device} from "../models/device";
import {BmeData} from "../models/bme-data";

@Injectable({
  providedIn: 'root'
})
export class StateService {
  devices: Map<string, Device> = new Map<string, Device>();
  bmeData: Map<string, WritableSignal<BmeData[]>> = new Map<string, WritableSignal<BmeData[]>>();
  motorPosition: Map<string, number> = new Map<string, number>();
  motorMoving: Map<string, boolean> = new Map<string, boolean>();

  constructor() {

  }
}
