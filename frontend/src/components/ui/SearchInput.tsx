import { InputHTMLAttributes, useEffect, useState } from "react";
import { Search } from "lucide-react";
import { cn } from "../../lib/utils";

interface SearchInputProps extends Omit<InputHTMLAttributes<HTMLInputElement>, "onChange"> {
  onSearch: (value: string) => void;
  debounceMs?: number;
  containerClassName?: string;
}

export function SearchInput({
  onSearch,
  debounceMs = 500,
  containerClassName,
  className,
  value: externalValue,
  ...props
}: SearchInputProps) {
  const [value, setValue] = useState(externalValue?.toString() || "");

  useEffect(() => {
    if (externalValue !== undefined) {
      setValue(externalValue.toString());
    }
  }, [externalValue]);

  useEffect(() => {
    const handler = setTimeout(() => {
      onSearch(value);
    }, debounceMs);

    return () => {
      clearTimeout(handler);
    };
  }, [value, debounceMs, onSearch]);

  return (
    <div className={cn("relative", containerClassName)}>
      <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
      <input
        type="search"
        value={value}
        onChange={(e) => setValue(e.target.value)}
        className={cn(
          "flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm transition-colors file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50 pl-9",
          className
        )}
        {...props}
      />
    </div>
  );
}
