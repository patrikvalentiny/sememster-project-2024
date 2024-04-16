import {inject, Injectable, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class DashboardService implements OnInit{
  private readonly http: HttpClient = inject(HttpClient);
  constructor() { }

  ngOnInit(): void {
    }
}
