export interface Librarian {
  id: string;
  username: string;
  email: string;
  isActive: boolean;
  branchId: string;
}

export interface CreateLibrarianRequest {
  username: string;
  email: string;
  password?: string;
}

export interface AssignLibrarianRequest {
  userId: string;
  branchId: string;
}
