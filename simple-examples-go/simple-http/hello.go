package main

import (
	"fmt"
	"log"
	"net/http"
)

func main() {
	log.Println("Starting server on :80...")
	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		fmt.Fprintf(w, "Hello, you've requested: %s\n", r.URL.Path)
	})

	if err := http.ListenAndServe(":3576", nil); err != nil {
		log.Fatalf("Server failed to start: %v", err)
	}
}
