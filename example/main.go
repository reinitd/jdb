package main

import (
	"fmt"

	jdb "github.com/reinitd/jdb"
)

func main() {
	db, err := jdb.NewJDB("./db/prim.json")
	if err != nil {
		fmt.Print(err)
	}

	fmt.Printf("DB path: %s", db.Path)
}
