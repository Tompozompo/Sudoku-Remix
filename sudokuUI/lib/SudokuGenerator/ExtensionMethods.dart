import 'dart:math';

extension IterableExtensions<T> on Iterable<T> {
  List<List<T>> subSetsOf() {
    if (this.isEmpty)
      return new List<List<T>>()..add(new List<T>());

    var element = this.take(1).toList();

    var haveNot = this.skip(1).toList().subSetsOf();
    var have = haveNot.map((set) => element..addAll(set as List<T>)).toList();  // this might be broken, not tested

    return have..addAll(haveNot as List<List<T>>);
  }
}

extension ListExtensions<T> on List<T> {
  void shuffle() {
    Random rnd = new Random();
    int n = this.length;
    while (n > 1) {
      n--;
      int k = rnd.nextInt(n+1);
      T value = this[k];
      this[k] = this[n];
      this[n] = value;
    }
  }
}
