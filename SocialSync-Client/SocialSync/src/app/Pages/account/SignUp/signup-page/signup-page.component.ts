import { Component, signal } from '@angular/core';
import { AbstractControl,FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ApiAccountService } from '../../../../core/services/api.services/api.account.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';


@Component({
  selector: 'app-signup-page',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './signup-page.component.html',
  styleUrl: './signup-page.component.scss'
})


export class SignupPageComponent {

  // private accountService = Inject(ApiAccountService)
  signupForm:FormGroup = new FormGroup({});
  formData = signal<any>(null);
  loading = signal(false);


  constructor(
    private fb:FormBuilder,
    private accountService:ApiAccountService,
    private snackBar: MatSnackBar,
    private router : Router
  ){
    this.initForm();
  }

  public initForm():void{
    this.signupForm = this.fb.nonNullable.group({
      email:['',[Validators.required,Validators.email]],
      username:['',[Validators.required]],
      password:['',[Validators.required,Validators.minLength(6),this.passwordStrengthValidator]],
      confirmPassword:['',[Validators.required]]
    },{validators:this.passwordMatchValidator})
  }



  passwordStrengthValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;
    if (!value) {
      return null;
    }
    const hasMinLength = value.length >= 6;
    const hasUpperCase = /[A-Z]/.test(value);
    const hasNumber = /[0-9]/.test(value);
    const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(value);
  
    const passwordValid = hasMinLength && hasUpperCase && hasNumber && hasSpecialChar;

    return passwordValid ? null: { passwordStrength: true };
  }



  passwordMatchValidator(control: AbstractControl): ValidationErrors | null  {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    if (password !== confirmPassword) {
      control.get('confirmPassword')?.setErrors({ passwordMismatch: true });
    }
    return null;
  }


    // Helper method to check specific password errors
    getPasswordErrors(): string[] {
      const control = this.signupForm.get('password');
      const errors: string[] = [];
  
      if (control?.touched && control.errors) {
        if (control.errors['required']) {
          errors.push('Password is required');
        }
        if (control.errors['minlength']) {
          console.log("hey dude")
          errors.push('Password must be at least 6 characters');
        }
        if (control.errors['passwordStrength']) {
          const strength = control.errors['passwordStrength'];
      
          if (!strength.hasUpperCase) {
            errors.push('Password must contain at least one uppercase letter');
          }
          if (!strength.hasLowerCase) {
            errors.push('Password must contain at least one lowercase letter');
          }
          if (!strength.hasNumeric) {
            errors.push('Password must contain at least one number');
          }
          if (!strength.hasSpecialChar) {
            errors.push('Password must contain at least one special character');
          }
        }
      }
      return errors;
    }
    

    openSnackBar(message: string) {
      this.snackBar.open(message, '❌', {
        verticalPosition: 'top',
        horizontalPosition: 'center'
      });
    }

    SuccessSnackBar(message:string){
      this.snackBar.open(message,'close', {
        duration: 2000, 
        verticalPosition: 'top',
        horizontalPosition: 'center'
      });
    }
    
    onSubmit(){
    
      if(!this.signupForm.invalid){
        this.formData.set(this.signupForm.value);
        this.loading.set(true);
        const payload = {
          UserName:this.signupForm.value['username'],
          Email : this.signupForm.value['email'],
          Password : this.signupForm.value['password']
        }
        this.accountService.signUp(payload).subscribe(
           {
            next: (response) => {
              console.log(response);
              this.loading.set(false);
              this.formData.set(null);
              this.SuccessSnackBar("User Registered Successfully!");
              this.router.navigate(['/login']);
            },
            error: (error) => {
              this.loading.set(false);
              if(error!=null && error.error!=null && error.error.errors.DuplicateUserName[0]!=null){
                 let error_msg = error.error.errors.DuplicateUserName[0];
                 this.openSnackBar(error_msg)
              }
             else{
                this.openSnackBar("Oops Something went wrong!");
              }
            },
           }
        )


      }
    }

  RedirectSignIn(){
    this.router.navigate(['/login']);
  }
} 
