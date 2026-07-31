import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { userService } from '@/lib/services/user.service';
import { branchService } from '@/lib/services/branch.service';
import { User } from '@/types/auth.types';
import { BranchDto } from '@/types/branch.types';
import { toast } from '@/components/ui/Toast';

const createLibrarianSchema = z.object({
  username: z.string().min(3, 'Username must be at least 3 characters'),
  email: z.string().email('Invalid email address'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
  branchId: z.string().optional(),
});

const assignBranchSchema = z.object({
  branchId: z.string().min(1, 'Please select a branch'),
});

type CreateFormData = z.infer<typeof createLibrarianSchema>;
type AssignFormData = z.infer<typeof assignBranchSchema>;

interface UserFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  user: User | null;
  onSuccess: () => void;
}

export function UserFormModal({ isOpen, onClose, user, onSuccess }: UserFormModalProps) {
  const [loading, setLoading] = useState(false);
  const [branches, setBranches] = useState<BranchDto[]>([]);

  const isEditMode = !!user;

  const {
    register: registerCreate,
    handleSubmit: handleCreateSubmit,
    reset: resetCreate,
    formState: { errors: createErrors },
  } = useForm<CreateFormData>({
    resolver: zodResolver(createLibrarianSchema),
  });

  const {
    register: registerAssign,
    handleSubmit: handleAssignSubmit,
    reset: resetAssign,
    setValue: setAssignValue,
    formState: { errors: assignErrors },
  } = useForm<AssignFormData>({
    resolver: zodResolver(assignBranchSchema),
  });

  useEffect(() => {
    if (isOpen) {
      void branchService.getAll(false).then((data) => setBranches(data)).catch(console.error);
      
      if (isEditMode && user) {
        setAssignValue('branchId', user.branchId || '');
      } else {
        resetCreate();
      }
    }
  }, [isOpen, isEditMode, user, setAssignValue, resetCreate]);

  const onCreate = async (data: CreateFormData) => {
    setLoading(true);
    try {
      await userService.createLibrarian({
        username: data.username,
        email: data.email,
        password: data.password,
        branchId: data.branchId || undefined,
      });
      toast.success('Librarian created successfully');
      onSuccess();
      onClose();
    } catch (error: any) {
      toast.error(error?.response?.data?.message || 'Failed to create librarian');
    } finally {
      setLoading(false);
    }
  };

  const onAssign = async (data: AssignFormData) => {
    if (!user) return;
    setLoading(true);
    try {
      await userService.assignBranch(user.id, data.branchId);
      toast.success('Branch assigned successfully');
      onSuccess();
      onClose();
    } catch (error: any) {
      toast.error(error?.response?.data?.message || 'Failed to assign branch');
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-950/80 backdrop-blur-sm">
      <div className="bg-slate-900 border border-slate-800 rounded-2xl w-full max-w-md overflow-hidden shadow-2xl">
        <div className="px-6 py-4 border-b border-slate-800 flex justify-between items-center">
          <div>
            <h3 className="text-lg font-semibold text-white">
              {isEditMode ? 'Assign Branch' : 'Add Librarian'}
            </h3>
            <p className="text-sm text-slate-400 mt-0.5">
              {isEditMode ? `Assign a branch to ${user?.username}` : 'Create a new librarian account'}
            </p>
          </div>
          <button onClick={onClose} className="text-slate-400 hover:text-white transition-colors">
            <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
        <div className="p-6">
      {isEditMode ? (
        <form onSubmit={handleAssignSubmit(onAssign)} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-slate-300 mb-1">Branch</label>
            <select
              {...registerAssign('branchId')}
              className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              <option value="">Select a branch...</option>
              {branches.map((b) => (
                <option key={b.id} value={b.id}>
                  {b.name}
                </option>
              ))}
            </select>
            {assignErrors.branchId && <p className="mt-1 text-sm text-red-400">{assignErrors.branchId.message}</p>}
          </div>

          <div className="flex justify-end gap-3 pt-4 border-t border-slate-800">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 text-sm font-medium text-slate-300 hover:text-white transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg text-sm font-medium transition-colors disabled:opacity-50"
            >
              {loading ? 'Saving...' : 'Assign Branch'}
            </button>
          </div>
        </form>
      ) : (
        <form onSubmit={handleCreateSubmit(onCreate)} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-slate-300 mb-1">Username</label>
            <input
              type="text"
              {...registerCreate('username')}
              className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
              placeholder="e.g. johndoe"
            />
            {createErrors.username && <p className="mt-1 text-sm text-red-400">{createErrors.username.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-300 mb-1">Email</label>
            <input
              type="email"
              {...registerCreate('email')}
              className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
              placeholder="john@example.com"
            />
            {createErrors.email && <p className="mt-1 text-sm text-red-400">{createErrors.email.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-300 mb-1">Password</label>
            <input
              type="password"
              {...registerCreate('password')}
              className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
              placeholder="Temporary password"
            />
            {createErrors.password && <p className="mt-1 text-sm text-red-400">{createErrors.password.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-300 mb-1">Assign Branch (Optional)</label>
            <select
              {...registerCreate('branchId')}
              className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              <option value="">No branch assigned initially</option>
              {branches.map((b) => (
                <option key={b.id} value={b.id}>
                  {b.name}
                </option>
              ))}
            </select>
          </div>

          <div className="flex justify-end gap-3 pt-4 border-t border-slate-800">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 text-sm font-medium text-slate-300 hover:text-white transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg text-sm font-medium transition-colors disabled:opacity-50"
            >
              {loading ? 'Creating...' : 'Create Librarian'}
            </button>
          </div>
        </form>
      )}
        </div>
      </div>
    </div>
  );
}
