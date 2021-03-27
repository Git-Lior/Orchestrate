import { useCallback, useState } from "react";

type ChangeCallback = (e?: { target: { value: string } }) => void;

export function useInputState(
  initialState: string | (() => string) = ""
): [string, ChangeCallback] {
  const [state, setState] = useState<string>(initialState);
  const changeState: ChangeCallback = useCallback(e => setState(e?.target.value ?? ""), [setState]);

  return [state, changeState];
}

export function useLocalStorage(
  storageKey: string
): [string | undefined, (value: string | undefined) => void] {
  const [value, setValue] = useState<string | undefined>(
    localStorage.getItem(storageKey) ?? undefined
  );

  const setStorageValue = useCallback(
    (newValue: string | undefined) => {
      if (!newValue) localStorage.removeItem(storageKey);
      else localStorage.setItem(storageKey, newValue);

      setValue(newValue);
    },
    [storageKey]
  );

  return [value, setStorageValue];
}
