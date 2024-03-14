export interface AddUserRequest {
  displayedName?: string;
  password: string;
  username: string;
}
export interface GetUserResponse {
  displayedName: string;
  id: number;
  username: string;
}
export interface SignInResponse {
  token: string;
}
export interface UpdateUserRequest {
  displayedNameUpdate?: string;
  passwordUpdate?: string;
}
