import React, { useCallback, useState } from "react";

type ChangeCallback = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => void;

export function useInputState(
  initialState: string | (() => string) = ""
): [string, ChangeCallback] {
  const [state, setState] = useState<string>(initialState);
  const changeState: ChangeCallback = useCallback(e => setState(e.target.value), [setState]);

  return [state, changeState];
}
