import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ApiAccountService } from '../../../../core/services/api.services/api.account.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss',
})
export class LoginPageComponent {
  signinForm: FormGroup = new FormGroup({});
  formData = signal<any>(null);
  loading = signal(false);

  constructor(
    private router: Router,
    private fb: FormBuilder,
    private accountService: ApiAccountService,
    private snackBar: MatSnackBar
  ) {
    this.initForm();
  }

  public initForm(): void {
    this.signinForm = this.fb.nonNullable.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  ErrorSnackBar(message: string) {
    this.snackBar.open(message, '❌', {
      verticalPosition: 'top',
      horizontalPosition: 'center',
    });
  }

  SuccessSnackBar(message: string) {
    this.snackBar.open(message, 'close', {
      verticalPosition: 'top',
      horizontalPosition: 'center',
    });
  }

  onSubmit() {
    if (!this.signinForm.invalid) {
      this.formData.set(this.signinForm.value);

      console.log(this.formData());

      if (this.formData()) {
        this.loading.set(true);
        let payload = this.formData();
        this.accountService.signIn(payload).subscribe({
          next: (response) => {
            this.loading.set(false);
            this.formData.set(null);
            if(response.data!=null && response.statusCode == 200){
              this.SuccessSnackBar('Login Success.')
            }else if(response.statusCode == 402){
              this.ErrorSnackBar('Please check your username and password.');
            }
         
          },
          error: (error) => {
       
            this.loading.set(false);
            this.ErrorSnackBar(
                'Oops Something went wrong!'
              );
          },
        });
      }
    }
  }

  RedirectSignup() {
    this.router.navigate(['/signup']);
  }
  RedirectforgotPassword() {
    this.router.navigate(['/forgotpassword']);
  }
}
