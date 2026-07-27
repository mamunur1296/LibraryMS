export interface BranchDto {
  id: string;
  name: string;
  address: string;
  phone: string;
  email: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateBranchRequest {
  name: string;
  address: string;
  phone: string;
  email: string;
}

export interface UpdateBranchRequest {
  id: string;
  name: string;
  address: string;
  phone: string;
  email: string;
}
