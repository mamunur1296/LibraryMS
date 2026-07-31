import { useEffect, useState } from "react";
import { branchService } from "../../lib/services/branch.service";
import { BranchDto } from "../../types/branch.types";
import { cn } from "../../lib/utils";
import { MapPin } from "lucide-react";

interface BranchSelectorProps {
  value?: string;
  onChange: (branchId: string) => void;
  className?: string;
  placeholder?: string;
  allowAll?: boolean;
}

export function BranchSelector({
  value,
  onChange,
  className,
  placeholder = "Select a branch",
  allowAll = false,
}: BranchSelectorProps) {
  const [branches, setBranches] = useState<BranchDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const fetchBranches = async () => {
      setIsLoading(true);
      try {
        const response = await branchService.getAll(true);
        setBranches(response);
      } catch (error) {
        console.error("Failed to load branches", error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchBranches();
  }, []);

  return (
    <div className={cn("relative", className)}>
      <MapPin className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
      <select
        value={value || ""}
        onChange={(e) => onChange(e.target.value)}
        disabled={isLoading}
        className="flex h-9 w-full appearance-none rounded-md border border-input bg-transparent pl-9 pr-8 py-1 text-sm shadow-sm transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
      >
        <option value="" disabled={!allowAll}>
          {isLoading ? "Loading..." : placeholder}
        </option>
        {allowAll && !isLoading && (
          <option value="all">All Branches</option>
        )}
        {branches.map((branch) => (
          <option key={branch.id} value={branch.id}>
            {branch.name}
          </option>
        ))}
      </select>
      <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2 text-muted-foreground">
        <svg className="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7"></path>
        </svg>
      </div>
    </div>
  );
}
