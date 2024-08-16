import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const tostar = inject(ToastrService);

  return next(req).pipe(
    catchError(error => {
      if(error) {
        switch (error.ststus) {
          case 400:
            if (error.error.errors) {
              const ModalStateErrors = [];
              for(const key in error.error.errors) {
                if(error.error.errors[key]) {
                  ModalStateErrors.push(error.error.errors[key])
                }
              }
              throw ModalStateErrors.flat();
            } else {
              tostar.error(error.error, error.ststus)
            }
            break;

            case 401:
              tostar.error('Unauthorised', error.ststus)
              break;

            case 404:
              router.navigateByUrl('/not-found');
              break;
              
            case 500:  
              const navigationExtras:NavigationExtras = {state: {error:error.error}};
              router.navigateByUrl('/server-error', navigationExtras);
              break;
        
          default:
            tostar.error('Something unexpected went wrong');
            break;
        }
      }
      throw error;
    })
  )
};
