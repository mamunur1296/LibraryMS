import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { registerSchema, RegisterFormData } from "@/lib/validations/auth.schema";
import { authService } from "@/lib/services/auth.service";
import { toast } from "@/components/ui/Toast";

export default function RegisterPage() {
  const navigate = useNavigate();
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      role: "Member", // Set default role as Member
    },
  });

  const onSubmit = async (data: RegisterFormData) => {
    setError("");
    setSuccess(false);
    try {
      await authService.register(data);
      setSuccess(true);
      toast.success("Account created successfully! Redirecting to login...");
      setTimeout(() => {
        navigate("/login");
      }, 2000);
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string } }; message?: string };
      const msg = axiosErr.response?.data?.message || axiosErr.message || "Failed to register";
      setError(msg);
      toast.error(msg);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-900 via-purple-900 to-slate-900">
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <div className="absolute -top-[30%] -left-[10%] w-[70%] h-[70%] rounded-full bg-purple-500/10 blur-3xl"></div>
        <div className="absolute top-[20%] -right-[20%] w-[60%] h-[60%] rounded-full bg-indigo-500/10 blur-3xl"></div>
      </div>

      <div className="relative w-full max-w-md p-8 bg-white/10 backdrop-blur-xl border border-white/20 shadow-2xl rounded-3xl z-10">
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-gradient-to-tr from-indigo-500 to-purple-500 shadow-lg mb-4">
            <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
            </svg>
          </div>
          <h2 className="text-3xl font-bold text-white tracking-tight">Create Account</h2>
          <p className="text-slate-300 mt-2 text-sm">Join the Library Management System</p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {error && (
            <div className="p-3 bg-red-500/20 border border-red-500/50 rounded-xl text-red-200 text-sm text-center animate-pulse">
              {error}
            </div>
          )}
          {success && (
            <div className="p-3 bg-emerald-500/20 border border-emerald-500/50 rounded-xl text-emerald-200 text-sm text-center">
              Registration successful! Redirecting to login...
            </div>
          )}

          <div className="space-y-1">
            <label className="text-sm font-medium text-slate-200 ml-1">Username</label>
            <input
              type="text"
              {...register("username")}
              className="w-full px-4 py-3 bg-slate-900/50 border border-slate-700/50 rounded-xl text-white placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
              placeholder="e.g. johndoe"
            />
            {errors.username && <p className="text-red-400 text-xs ml-1 mt-1">{errors.username.message}</p>}
          </div>

          <div className="space-y-1">
            <label className="text-sm font-medium text-slate-200 ml-1">Email</label>
            <input
              type="email"
              {...register("email")}
              className="w-full px-4 py-3 bg-slate-900/50 border border-slate-700/50 rounded-xl text-white placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
              placeholder="johndoe@example.com"
            />
            {errors.email && <p className="text-red-400 text-xs ml-1 mt-1">{errors.email.message}</p>}
          </div>

          <div className="space-y-1">
            <label className="text-sm font-medium text-slate-200 ml-1">Password</label>
            <input
              type="password"
              {...register("password")}
              className="w-full px-4 py-3 bg-slate-900/50 border border-slate-700/50 rounded-xl text-white placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
              placeholder="••••••••"
            />
            {errors.password && <p className="text-red-400 text-xs ml-1 mt-1">{errors.password.message}</p>}
          </div>

          <div className="space-y-1">
            <label className="text-sm font-medium text-slate-200 ml-1">Role</label>
            <select
              {...register("role")}
              className="w-full px-4 py-3 bg-slate-900/50 border border-slate-700/50 rounded-xl text-slate-300 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
            >
              <option value="Member" className="bg-slate-900 text-white">Member</option>
              <option value="Librarian" className="bg-slate-900 text-white">Librarian</option>
              <option value="Admin" className="bg-slate-900 text-white">Admin</option>
            </select>
            {errors.role && <p className="text-red-400 text-xs ml-1 mt-1">{errors.role.message}</p>}
          </div>

          <div className="pt-4">
            <button
              type="submit"
              disabled={isSubmitting || success}
              className="w-full py-3 px-4 bg-gradient-to-r from-indigo-500 to-purple-600 hover:from-indigo-400 hover:to-purple-500 text-white font-medium rounded-xl shadow-lg shadow-indigo-500/30 transform hover:-translate-y-0.5 transition-all duration-200 disabled:opacity-70 disabled:cursor-not-allowed flex items-center justify-center"
            >
              {isSubmitting ? (
                <svg className="animate-spin -ml-1 mr-2 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
              ) : null}
              {isSubmitting ? "Creating Account..." : "Create Account"}
            </button>
          </div>

          <div className="text-center mt-4">
            <p className="text-sm text-slate-400">
              Already have an account?{" "}
              <Link to="/login" className="text-indigo-400 hover:text-indigo-300 font-medium">
                Sign in
              </Link>
            </p>
          </div>
        </form>
      </div>
    </div>
  );
}
