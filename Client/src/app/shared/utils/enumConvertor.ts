const capitalizeWords = (str: string) =>
  str.replace(/\b\w/g, (char) => char.toUpperCase());

const prettyLabel = (key: string) =>
  capitalizeWords(key.replace(/([a-z])([A-Z])/g, '$1 $2'));

export function getEnumOptions<T extends Record<string, string | number>>(
  enumObj: T
) {
  return Object.entries(enumObj)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({
      value: value as number,
      label: prettyLabel(key),
    }));
}

export function getEnumLabel<T extends Record<string, string | number>>(
  enumObj: T,
  value: number | string
): string | undefined {
  const key = Object.keys(enumObj).find((k) => enumObj[k as keyof T] === value);
  return key ? prettyLabel(key) : undefined;
}
