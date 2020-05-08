import { AuthLogin } from "./auth-login";

export class UserRegister extends AuthLogin {
  userName: string;
  confirmPassword: string;
  constructor(
    userName: string,
    email: string,
    password: string,
    confirmPassword: string
) {
  super();
  this.confirmPassword=confirmPassword;
  this.email=email;
  this.password=password;
  this.userName=userName;
}
}
