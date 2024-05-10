import {inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {firstValueFrom} from "rxjs";
import {StateService} from "./state.service";
import {HotToastService} from "@ngxpert/hot-toast";
import {MotorPositionDto} from "../models/motor-position-dto";

@Injectable({
  providedIn: 'root'
})
export class MotorService {
  private readonly http = inject(HttpClient);
  private readonly toast = inject(HotToastService);
  private readonly stateService = inject(StateService);

  constructor() {
  }

  public async getMotorPosition(mac: string) {
    try {
      const call = this.http.get<MotorPositionDto>(`${environment.restBaseUrl}/device/${mac}/motor`)
      const response = await firstValueFrom<MotorPositionDto>(call);
      this.stateService.motorPosition.set(mac, response.lastMotorPosition);
      return response;
    } catch (e) {
      this.toast.error("Failed to get motor position");
      throw e;
    }
  }

  public async setMaxPosition(mac: string, position: number) {
    try {
      const call = this.http.put<number>(`${environment.restBaseUrl}/device/${mac}/motor`, position)
      const response = await firstValueFrom<number>(call);
      this.stateService.motorPosition.set(mac, response);
      return response;
    } catch (e) {
      this.toast.error("Failed to set max position");
      throw e;
    }
  }
}
