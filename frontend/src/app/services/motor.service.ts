import {inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {firstValueFrom} from "rxjs";
import {StateService} from "./state.service";
import {MotorPositionDto} from "../models/motor-position-dto";

@Injectable({
  providedIn: 'root'
})
export class MotorService {
  private readonly http = inject(HttpClient);
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
      throw e;
    }
  }

  public async setMaxPosition(mac: string, position: number) {
    try {
      const call = this.http.put<number>(`${environment.restBaseUrl}/device/${mac}/motor`, position, {headers:{'Content-Type': 'application/json'}})
      const response = await firstValueFrom<number>(call);
      this.stateService.motorPosition.set(mac, response);
      return response;
    } catch (e) {
      throw e;
    }
  }

  public async getMotorDirection(mac: string) {
    try {
      const call = this.http.get<boolean>(`${environment.restBaseUrl}/device/${mac}/motor-direction`)
      return await firstValueFrom<boolean>(call);
    } catch (e) {
      throw e;
    }
  }

  public async setMotorDirection(mac: string, direction: boolean) {
    try {
      const call = this.http.put<boolean>(`${environment.restBaseUrl}/device/${mac}/motor-direction`, direction)
      return await firstValueFrom<boolean>(call);
    } catch (e) {
      throw e;
    }
  }
}
