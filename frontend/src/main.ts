import {importProvidersFrom} from '@angular/core';
import {AppComponent} from './app/app.component';
import {bootstrapApplication, BrowserModule} from '@angular/platform-browser';
import {provideHotToastConfig} from "@ngxpert/hot-toast";
import {provideHttpClient, withFetch} from "@angular/common/http";
import {provideRouter, Routes, withComponentInputBinding} from "@angular/router";
import {DashboardComponent} from "./app/pages/home/dashboard/dashboard.component";
import {MotorControlComponent} from "./app/pages/motor-control/motor-control.component";

const routes: Routes = [
  {
    path: '',
    component: DashboardComponent
  },
  {
    path: 'motor/:mac',
    component: MotorControlComponent
  }
];

bootstrapApplication(AppComponent, {
  providers: [importProvidersFrom(BrowserModule), provideHttpClient(withFetch()),
    provideHotToastConfig(
      {
        duration: 5000,
        position: 'bottom-center',
        style: {
          backgroundColor: 'oklch(var(--b3))',
          color: 'oklch(var(--bc))',
        }
      }
    )
  ,
    provideRouter(routes,
      withComponentInputBinding()
    )]
})
  .catch(err => console.error(err));
