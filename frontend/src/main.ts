import {importProvidersFrom} from '@angular/core';
import {AppComponent} from './app/app.component';
import {bootstrapApplication, BrowserModule} from '@angular/platform-browser';
import {provideHotToastConfig} from "@ngxpert/hot-toast";
import {HTTP_INTERCEPTORS, provideHttpClient, withFetch} from "@angular/common/http";
import {provideRouter, Routes, withComponentInputBinding} from "@angular/router";
import {DashboardComponent} from "./app/pages/home/dashboard/dashboard.component";
import {MotorControlComponent} from "./app/pages/motor-control/motor-control.component";
import {ErrorHttpInterceptor} from "./app/interceptors/error-http-interceptor";
import {HistoricDataComponent} from "./app/pages/historic-data/historic-data.component";

const routes: Routes = [
  {
    path: '',
    component: DashboardComponent
  },
  {
    path: 'motor/:mac',
    component: MotorControlComponent
  },
  {
    path: 'data/:mac',
    component: HistoricDataComponent
  },
  {
    path: '**',
    redirectTo: ''
  },

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
    ),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorHttpInterceptor,
      multi: true
    },
  ]
})
  .catch(err => console.error(err));
