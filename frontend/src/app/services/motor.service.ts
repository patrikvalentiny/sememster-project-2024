import {inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Device} from "../models/device";
import {environment} from "../../environments/environment";
import {firstValueFrom} from "rxjs";
import {StateService} from "./state.service";
import {HotToastService} from "@ngxpert/hot-toast";

@Injectable({
  providedIn: 'root'
})
export class MotorService {
  private readonly http = inject(HttpClient);
  private readonly toast = inject(HotToastService);
  private readonly stateService = inject(StateService);

  constructor() { }

  public async getMotorPosition(mac: string) {
    try{


    const call = this.http.get<number>(`${environment.restBaseUrl}/device/${mac}/motor`)
    const response = await firstValueFrom<number>(call);
    this.stateService.motorPosition.set(mac, response);
    return response;
    } catch (e){
      this.toast.error("Failed to get motor position");
      throw e;
    }
  }
}
