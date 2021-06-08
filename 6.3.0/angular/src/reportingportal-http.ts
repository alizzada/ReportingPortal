import { Injectable, Injector } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest, HttpEvent, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { AbpHttpConfigurationService, AbpHttpInterceptor, TokenService } from 'abp-ng2-module';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';


@Injectable()
export class ReportingInterceptor implements HttpInterceptor {

    constructor(private _tokenService: TokenService) {
    }


    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        request = request.clone({
            setHeaders: {
                'Bearer': `${this._tokenService.getToken()}`
            }
        });

        return next.handle(request).pipe(
            catchError((response: HttpErrorResponse) => {
                if (response.status == 401) {
                    /* location.href = "https://www.w3schools.com";*/
                    console.error('adad');

                }
                return throwError(response);
            })
        )
    }
}
