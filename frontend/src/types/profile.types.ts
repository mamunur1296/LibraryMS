export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface ChangeUsernameRequest {
  newUsername: string;
}

export interface ChangeEmailRequest {
  newEmail: string;
}
