import {inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Device} from "../models/device";
import {environment} from "../../environments/environment";
import {firstValueFrom} from "rxjs";
import {StateService} from "./state.service";

@Injectable({
  providedIn: 'root'
})
export class MotorService {
  private readonly http = inject(HttpClient);
  private readonly stateService = inject(StateService);

  constructor() { }

  public async getMotorPosition(mac: string) {
    const call = this.http.get<number>(`${environment.restBaseUrl}/device/${mac}/motor`)
    const response = await firstValueFrom<number>(call);
    this.stateService.motorPosition.set(mac, response);
    return response;
  }
}
