import {HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from "@angular/common/http";
import {inject, Injectable} from "@angular/core";
import {catchError, Observable} from "rxjs";
import {HotToastService} from "@ngxpert/hot-toast";

@Injectable()
export class ErrorHttpInterceptor implements HttpInterceptor {
  private readonly toastService = inject(HotToastService);

  intercept(req: HttpRequest<any>, next: HttpHandler):
    Observable<HttpEvent<any>> {
    return next.handle(req).pipe(catchError(async e => {
      if (e instanceof HttpErrorResponse) {
        switch (e.status) {
          case 401:
            await this.showError("You are not authorized to access this page. Please log in or check your credentials.");
            break;
          case 403:
            await this.showError("Sorry, you don't have permission to access this resource. Please contact the administrator.");
            break;
          case 404:
            await this.showError("Oops! The page or resource you're looking for doesn't exist. Please verify the URL or navigate elsewhere.");
            break;
          case 500:
            await this.showError("Something went wrong on our end. We're working to fix it. Please try again later.");
            break;
          case 503:
            await this.showError("Oops! The server is not ready to handle the request. Please try again later.");
            break;
          case 400:
            if (e.error.title === "One or more validation errors occurred.") {
              await this.showError("There has been an error with your input. Please fill in all required fields");
              break;
            }
            await this.showError(e.error);
            break;
          case 0:
            await this.showError("The server is not responding");
            break;
        }
      }

      throw e;
    }));
  }

  private async showError(message: string) {
    return this.toastService.error(message)
  }
}
