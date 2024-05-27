import {inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {environment} from "../../environments/environment";
import {BmeData} from "../models/bme-data";

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private readonly http: HttpClient = inject(HttpClient);

  constructor() {
  }

  public async getDataPastXDays(mac: string, days: number) {
    try {
      const call = this.http.get<BmeData[]>(`${environment.restBaseUrl}/data/${mac}?days=${days}`);
      return await firstValueFrom<BmeData[]>(call);
    } catch (e) {
      throw e;
    }
  }
}
