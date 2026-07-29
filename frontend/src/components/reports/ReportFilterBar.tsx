import { useEffect, useState } from "react";
import { branchService } from "@/lib/services/branch.service";

interface ReportFilterBarProps {
  onFilterChange: (filters: { fromDate?: string; toDate?: string; branchId?: string }) => void;
  showBranchFilter?: boolean;
}

export function ReportFilterBar({ onFilterChange, showBranchFilter = true }: ReportFilterBarProps) {
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [branchId, setBranchId] = useState("");
  const [branches, setBranches] = useState<any[]>([]);

  useEffect(() => {
    if (showBranchFilter) {
      branchService.getAll(false).then(setBranches).catch(console.error);
    }
  }, [showBranchFilter]);

  const handleApply = () => {
    onFilterChange({
      fromDate: fromDate || undefined,
      toDate: toDate || undefined,
      branchId: branchId || undefined,
    });
  };

  const handleClear = () => {
    setFromDate("");
    setToDate("");
    setBranchId("");
    onFilterChange({
      fromDate: undefined,
      toDate: undefined,
      branchId: undefined,
    });
  };

  return (
    <div className="bg-slate-900 border border-slate-800 rounded-xl p-4 flex flex-col md:flex-row items-end gap-4 shadow-sm">
      <div className="w-full md:w-auto flex-1 grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label className="block text-xs font-medium text-slate-400 mb-1">From Date</label>
          <input
            type="date"
            value={fromDate}
            onChange={(e) => setFromDate(e.target.value)}
            className="w-full px-3 py-2 bg-slate-950 border border-slate-700 rounded-lg text-slate-300 focus:outline-none focus:ring-2 focus:ring-indigo-500 text-sm"
          />
        </div>
        <div>
          <label className="block text-xs font-medium text-slate-400 mb-1">To Date</label>
          <input
            type="date"
            value={toDate}
            onChange={(e) => setToDate(e.target.value)}
            className="w-full px-3 py-2 bg-slate-950 border border-slate-700 rounded-lg text-slate-300 focus:outline-none focus:ring-2 focus:ring-indigo-500 text-sm"
          />
        </div>
        {showBranchFilter && (
          <div>
            <label className="block text-xs font-medium text-slate-400 mb-1">Branch</label>
            <select
              value={branchId}
              onChange={(e) => setBranchId(e.target.value)}
              className="w-full px-3 py-2 bg-slate-950 border border-slate-700 rounded-lg text-slate-300 focus:outline-none focus:ring-2 focus:ring-indigo-500 text-sm"
            >
              <option value="">All Branches</option>
              {branches.map(b => (
                <option key={b.id} value={b.id}>{b.name}</option>
              ))}
            </select>
          </div>
        )}
      </div>
      
      <div className="flex gap-2 w-full md:w-auto">
        <button
          onClick={handleClear}
          className="flex-1 md:flex-none px-4 py-2 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded-lg text-sm font-medium transition-colors"
        >
          Clear
        </button>
        <button
          onClick={handleApply}
          className="flex-1 md:flex-none px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg text-sm font-medium transition-colors shadow-lg shadow-indigo-500/20"
        >
          Apply Filters
        </button>
      </div>
    </div>
  );
}
